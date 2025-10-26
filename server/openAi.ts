import { OpenAI } from "openai";
import { zodTextFormat } from "openai/helpers/zod";
import { z } from "zod";

const OPENAI_API_KEY = process.env.OPENAI_API_KEY;

export interface Message {
    role: "system" | "user" | "assistant";
    content: string;
}

const MoveDescriptions = z.object({
    move_1: z.string().describe("The first move description for the player's action"),
    move_2: z.string().describe("The second move description for the player's action"),
    move_3: z.string().describe("The third move description for the player's action"),
    move_4: z.string().describe("The fourth move description for the player's action")
}).strict();

const WhoWon = z.object({
    winner: z.enum(["player_1", "player_2"]).describe("The player who won the match")
}).strict();

class OpenAiService {
    private client: OpenAI;

    constructor() {
        this.client = new OpenAI({
            apiKey: OPENAI_API_KEY,
        });

    }
    
    private async prompt(messages: Message[]) {
        const response = await this.client.responses.create({
            model: "gpt-5",
            input: messages as any
        });

        return response.output_text;
    }

    async generate_moves(messages: Message[]) {
        const response = await this.client.responses.parse({
            model: "gpt-5",
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