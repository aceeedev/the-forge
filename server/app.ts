import express from 'express'
import { openAiService , type Message} from './openAi'
import { decartService } from './decart'
const app = express()

app.use(express.json())


app.get('/new-conversation', async (req, res) => {
    await openAiService.new_conversation()
    res.sendStatus(200)
})

app.get('/prompt-response', async (req, res) => {
    const text = req.query.text as string
    const messages = [{ role: "user", content: text }] as Message[]
    const response = await openAiService.prompt(messages)
    res.send({ response })
})

app.get('/generate-move-descriptions', async (req, res) => {
    const text = req.query.text as string
    const messages = [{ role: "user", content: text }] as Message[]
    const response = await openAiService.generate_moves(messages)
    res.send({ response })
})

app.get('/decide-winner', async (req, res) => {
    const text = req.query.text as string
    const messages = [{ role: "user", content: text }] as Message[]
    const response = await openAiService.decide_winner(messages)
    res.send({ response })
})

app.get('/final-winner', async (req, res) => {
    try {
        const text = req.query.text as string
        console.log("final-winner endpoint called with text:", text)
        const messages = [{ role: "user", content: text }] as Message[]
        const response = await openAiService.final_winner(messages)
        console.log("final_winner returned:", response)
        
        if (!response || response === "null") {
            console.error("Response is null or empty")
            return res.status(500).send({ error: "AI returned no response" })
        }
        
        res.send({ response })
    } catch (error) {
        console.error("Error in final-winner endpoint:", error)
        res.status(500).send({ error: String(error) })
    }
})

app.get('/generate-image', async (req, res) => {
    const prompt = req.query.text as string
    const imageBuffer = await decartService.generateImage(prompt)
    res.setHeader('Content-Type', 'image/png')
    res.send(imageBuffer)
})

// app.post('/edit-image', async (req, res) => {
//     const prompt = req.body.text as string
//     const image = req.body.image as Buffer
//     const imageBuffer = await decartService.editImage(prompt, image)
//     res.setHeader('Content-Type', 'image/png')
//     res.send(imageBuffer)
// })

app.listen(3000, () => {
    console.log('Server running on http://localhost:3000')
})