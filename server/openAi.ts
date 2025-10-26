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
    move_1: z.string().describe("A brief less than 8 word setnence description for the player's potential action and effect using the item they have in a way that clearly progress the story, aligning with a particular tactic and clearly signifying how the character trait is used."),
    move_2: z.string().describe("A brief less than 8 word setnence description for the player's potential action and effect using the talent they have in a way that clearly progress the story, aligning with a particular tactic and clearly signifying how the character trait is used."),
    move_3: z.string().describe("A brief less than 8 word setnence description for the player's potential action and effect using the clothes they have in a way that clearly progress the story, aligning with a particular tactic and clearly signifying how the character trait is used."),
    move_4: z.string().describe("A brief less than 8 word setnence description for the player's potential action and effect using the ability they have in a way that clearly progress the story, aligning with a particular tactic and clearly signifying how the character trait is used."),
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
                    Your task is to describe the unfolding story based on the players' actions. When receiving the situation, you can expand on it slightly to add additional details.
                    Be realistic about the practicality and consequences of the players' choices, but still allow for creativity and imagination.
                    Ensure the story is told clearly, coherently, and is easy for players to follow.
                    Diversify the actions players can take and focus on diversifying personailities and tactics to choose fromsuch as defense, stealth, efficiency, and chaos.
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