import { useState } from "react";

function App() {
  const [prompt, setPrompt] = useState("");
  const [result, setResult] = useState("");

  const generateCharacter = async () => {
  const response = await fetch("http://localhost:5068/api/character/generate", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      prompt: prompt,
    }),
  });

  const data = await response.json();

  setResult(data);
};

  return (
    <>
      <h1>AI Character Generator</h1>

      <textarea
        value={prompt}
        onChange={(event) => setPrompt(event.target.value)}
        placeholder="Beskriv din karaktär..."
      />

      <button onClick={generateCharacter}>
  Skapa karaktär
</button>
      <p>{result}</p>
    </>
  );
}

export default App;