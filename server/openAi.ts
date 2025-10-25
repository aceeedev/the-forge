import { OpenAI } from "openai";

const OPENAI_API_KEY = process.env.OPENAI_API_KEY;

export interface Message {
    role: "system" | "user" | "assistant";
    content: string;
}

class OpenAiService {
    private client: OpenAI;

    constructor() {
        this.client = new OpenAI({
            apiKey: OPENAI_API_KEY,
        });

    }
    
    async prompt(messages: Message[]) {
        const response = await this.client.responses.create({
            model: "gpt-5",
            input: messages as any
        });

        return response.output_text;
    }
}

export const openAiService = new OpenAiService();