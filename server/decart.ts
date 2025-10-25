const DECART_API_KEY = process.env.DECART_API_KEY;
const DECART_API_IMAGE_GENERATE_URL = "https://api.decart.ai/v1/generate/lucy-pro-t2i";
const DECART_API_IMAGE_EDIT_URL = "https://api.decart.ai/v1/generate/lucy-pro-i2i";


class DecartService {
    async generateImage(prompt: string): Promise<Buffer> {
        const response = await fetch(
            DECART_API_IMAGE_GENERATE_URL,
            {
                method: 'POST',
                headers: {
                    'x-api-key': DECART_API_KEY as string,
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: new URLSearchParams({prompt: prompt})
            }
        );
        const arrayBuffer = await response.arrayBuffer();
        return Buffer.from(arrayBuffer);
    }

    async editImage(prompt: string, image: Buffer) {
        // const imageBuffer = await this.generateImage("A man riding a hog");
        
        const formData = new FormData();
        const blob = new Blob([new Uint8Array(image)], { type: 'image/png' });
        formData.append("data", blob, "image.png");
        formData.append("prompt", prompt);
        formData.append("resolution", "720p");

        const response = await fetch(
            DECART_API_IMAGE_EDIT_URL,
            {
                method: 'POST',
                headers: {
                    'x-api-key': DECART_API_KEY as string,
                },
                body: formData
            }
        );
        
        const arrayBuffer = await response.arrayBuffer();
        return Buffer.from(arrayBuffer);
    }
}

export const decartService = new DecartService();