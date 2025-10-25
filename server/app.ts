import express from 'express'
import { openAiService , type Message} from './openAi'
import { decartService } from './decart'
const app = express()

app.use(express.json())

app.post('/generate-move-descriptions', async (req, res) => {
    const messages = req.body.messages as Message[]
    const response = await openAiService.generate_moves(messages)
    res.send({ response })
})

app.post('/decide-winner', async (req, res) => {
    const messages = req.body.messages as Message[]
    const response = await openAiService.decide_winner(messages)
    res.send({ response })
})

app.post('/generate-image', async (req, res) => {
    const prompt = req.body.prompt as string
    const imageBuffer = await decartService.generateImage(prompt)
    res.setHeader('Content-Type', 'image/png')
    res.send(imageBuffer)
})

app.post('/edit-image', async (req, res) => {
    const prompt = req.body.prompt as string
    const image = req.body.image as Buffer
    const imageBuffer = await decartService.editImage(prompt, image)
    res.setHeader('Content-Type', 'image/png')
    res.send(imageBuffer)
})

app.listen(3000, () => {
    console.log('Server running on http://localhost:3000')
})