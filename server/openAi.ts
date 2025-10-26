import { OpenAI } from "openai";
import { zodTextFormat } from "openai/helpers/zod";
import { Conversation } from "openai/resources/conversations/conversations.mjs";
import { z } from "zod";

const OPENAI_API_KEY = process.env.OPENAI_API_KEY;

export interface Message {
    role: "system" | "user" | "assistant";
    content: string;
}

const MoveDescriptions = z.object({
    move_1: z.string().describe("A very brief description for the player's potential action using the item they have in a way that clearly progress the story. End with the alignment of the action in brackets, like [Defensive]."),
    move_2: z.string().describe("A very brief description for the player's potential action using the talent they have in a way that clearly progress the story. End with the alignment of the action in brackets, like [Defensive]."),
    move_3: z.string().describe("A very brief description for the player's potential action using the clothes they have in a way that clearly progress the story. End with the alignment of the action in brackets, like [Defensive]."),
    move_4: z.string().describe("A very brief description for the player's potential action using the ability they have in a way that clearly progress the story. End with the alignment of the action in brackets, like [Defensive]."),
}).strict();

const WhoWon = z.object({
    winner: z.enum(["player_1", "player_2"]).describe("The player who won the match")
}).strict();

class OpenAiService {
    private client: OpenAI;
    private conversation: Conversation | null = null;

    constructor() {
        this.client = new OpenAI({
            apiKey: OPENAI_API_KEY,
        });
        // see https://platform.openai.com/docs/guides/conversation-state#openai-apis-for-conversation-state
        setTimeout(async () => {
            this.conversation = await this.client.conversations.create();
            this.client.responses.create({
                conversation: this.conversation.id,
                model: "gpt-4.1-mini",
                input: `
                    You are the game master narrating a story where players compete to overcome a given situation.
                    The situation and player details (including their items, talents, clothing, and abilities) will be provided later.
                    Your task is to describe the unfolding story based on the players' actions.
                    Be realistic about the practicality and consequences of their choices, while still allowing space for creativity and imagination.
                    Ensure the story is told clearly, coherently, and is easy for players to follow.
                    You will be asked to give the players 4 potential actions to progress the story that should be realstic but creative, diversify each action and make them different from each other.
                    The choice ofactions should focus on different personailities and tactics such as defense, stealth, efficiency, and chaos.
                `
            });
            console.log("Conversation initiated!!!");
        }, 0);
    }
    
    public async prompt(messages: Message[]) {
        if (this.conversation == null) {
            throw new Error("Conversation not initiated");
        }
        const response = await this.client.responses.create({
            conversation: this.conversation.id,
            model: "gpt-4.1-mini",
            input: messages as any
        });

        return response.output_text;
    }
    
    async generate_moves(messages: Message[]) {
        if (this.conversation == null) {
            throw new Error("Conversation not initiated");
        }
        const response = await this.client.responses.parse({
            conversation: this.conversation.id,
            model: "gpt-4.1-mini",
            input: messages as any,
            text: {
                format: zodTextFormat(MoveDescriptions, "event"),
            },
        });

        return response.output_text;
    }

    async decide_winner(messages: Message[]) {
        const response = await this.client.responses.parse({
            model: "gpt-5",
            input: messages as any,
            text: {
                format: zodTextFormat(WhoWon, "event"),
            },
        });

        return response.output_text;
    }
}

export const openAiService = new OpenAiService();