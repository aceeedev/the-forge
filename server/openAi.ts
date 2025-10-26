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
    move_1: z.string().describe("A brief less than 8 word sentence description for the player's potential action and effect using the item they have in a way that clearly progress the story, aligning with a particular tactic (like stealth, efficiency, or chaos) and clearly signifying how the character trait is used."),
    move_2: z.string().describe("A brief less than 8 word sentence description for the player's potential action and effect using the talent they have in a way that clearly progress the story, aligning with a particular tactic (like stealth, efficiency, or chaos) and clearly signifying how the character trait is used."),
    move_3: z.string().describe("A brief less than 8 word sentence description for the player's potential action and effect using the clothes they have in a way that clearly progress the story, aligning with a particular tactic (like stealth, efficiency, or chaos) and clearly signifying how the character trait is used."),
    move_4: z.string().describe("A brief less than 8 word sentence description for the player's potential action and effect using the ability they have in a way that clearly progress the story, aligning with a particular tactic (like stealth, efficiency, or chaos) and clearly signifying how the character trait is used."),
}).strict();

const WhoWon = z.object({
    winner: z.enum(["player_1", "player_2"]).describe("The player who won the match"),
    explanation: z.string().describe("A very detailed explanation of why the winner won this stage of the game.")
}).strict();

const FinalWinner = z.object({
    award_1_winner: z.string().describe("In a sentence no more than 10 words, detail which player won the match by winning the most situations and any key traits that made them win."),
    award_2_winner: z.string().describe("In a sentence no more than 10 words, detail which player made the best move in just one situation, explaining how it happened and at which point in the game they did it."),
}).strict();

class OpenAiService {
    private client: OpenAI;
    private conversation: Conversation | null = null;

    constructor() {
        this.client = new OpenAI({
            apiKey: OPENAI_API_KEY,
        });
        // see https://platform.openai.com/docs/guides/conversation-state#openai-apis-for-conversation-state
       this.new_conversation();
    }

    async new_conversation() {
        this.conversation = null;
        this.conversation = await this.client.conversations.create();
        this.client.responses.create({
            conversation: this.conversation.id,
            model: "gpt-4.1-mini",
            input: `
                You are the Game Master, narrating a story where players compete to overcome a situation independently.
                The situation and player details (including their items, talents, clothing, and abilities) will be provided later.
                Your role is to describe the unfolding story as players choose 1 of 4 actions you proposein response to the situation.
                Be realistic about the practicality and consequences of each player's actions, be sure to punish actions that realistically fail, but look for creativity and unexpected problem-solving.
                Actions for the players should be diverse and reflect a variety of personalities, motivations, and strategies—such as stealth, efficiency, or chaos.
                Be sure to remember players' actions as you will decide who the winner is based on their storylines.
                Do not ask for additional details; all necessary information will be provided when needed.
            `
        });
        console.log("New conversation initiated!!!");
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

        // Return the parsed data as a JSON string
        return JSON.stringify(response.output_parsed);
    }

    async decide_winner(messages: Message[]) {
        if (this.conversation == null) {
            throw new Error("Conversation not initiated");
        }
        const response = await this.client.responses.parse({
            conversation: this.conversation.id,
            model: "gpt-4.1-mini",
            input: messages as any,
            text: {
                format: zodTextFormat(WhoWon, "event"),
            },
        });

        // Return the parsed data as a JSON string
        return JSON.stringify(response.output_parsed);
    }

    async final_winner(messages: Message[]) {
        if (this.conversation == null) {
            throw new Error("Conversation not initiated");
        }
        console.log("final_winner called with messages:", messages);
        const response = await this.client.responses.parse({
            conversation: this.conversation.id,
            model: "gpt-4.1-mini",
            input: messages as any,
            text: { 
                format: zodTextFormat(FinalWinner, "event"),
            },
        });
        return JSON.stringify(response.output_parsed);
    }
}

export const openAiService = new OpenAiService();