import React, { useState, useRef } from "react";
import type { DriverWithGap } from "./RaceContext";
import axios from "axios";

type Props = {
  drivers: DriverWithGap[];
};

const AudioTranscription: React.FC<Props> = ({ drivers }) => {
  const [recording, setRecording] = useState(false);
  const [transcription, setTranscription] = useState("");
  const [aiResponse, setAIResponse] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const mediaRecorderRef = useRef<MediaRecorder | null>(null);
  const audioChunks = useRef<Blob[]>([]);

  const startRecording = async () => {
    setError("");
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      const mediaRecorder = new MediaRecorder(stream);
      mediaRecorderRef.current = mediaRecorder;
      audioChunks.current = [];

      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          audioChunks.current.push(event.data);
        }
      };

      mediaRecorder.onstop = async () => {
        const audioBlob = new Blob(audioChunks.current, { type: "audio/wav" });

        if (audioBlob.size === 0) {
          setError("Recorded audio is empty.");
          return;
        }

        const audioFile = new File([audioBlob], "audio.wav", {
          type: "audio/wav",
        });
        const formData = new FormData();
        formData.append("AudioFile", audioFile);

        const raceJson = {
          drivers: drivers.map((d) => ({
            id: d.driver.id,
            name: d.driver.name,
            team: d.driver.team,
            abr: d.driver.abbreviation,
            gap:
              d.gapFromLeader === "First"
                ? "first"
                : parseFloat(d.gapFromLeader).toString(),
          })),
        };
        formData.append("RaceJson", JSON.stringify(raceJson));

        try {
          setIsLoading(true);
          const response = await axios.post<{
            transcript: string;
            aiResponse: string;
          }>("https://localhost:7028/api/Speech/transcribe", formData, {
            headers: { "Content-Type": "multipart/form-data" },
          });

          setTranscription(response.data.transcript);
          setAIResponse(response.data.aiResponse);
        } catch (err) {
          console.error("Error during transcription:", err);
          setError("Error occurred during transcription.");
        } finally {
          setIsLoading(false);
        }
      };

      mediaRecorder.start();
      setRecording(true);
    } catch (err) {
      console.error("Microphone access denied or not available", err);
      setError("Microphone access denied or not available.");
    }
  };

  const stopRecording = () => {
    if (
      mediaRecorderRef.current &&
      mediaRecorderRef.current.state !== "inactive"
    ) {
      mediaRecorderRef.current.stop();
      setRecording(false);
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "600px", margin: "auto" }}>
      <h2>AI Race Engineer</h2>

      {recording ? (
        <button onClick={stopRecording} disabled={isLoading}>
          Stop Recording
        </button>
      ) : (
        <button onClick={startRecording} disabled={isLoading}>
          Start Recording
        </button>
      )}

      {isLoading && <p>Transcribing and thinking...</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}

      {transcription && (
        <div>
          <h3>Transcript:</h3>
          <p>{transcription}</p>
        </div>
      )}

      {aiResponse && (
        <div>
          <h3>Engineer:</h3>
          <p>{aiResponse}</p>
        </div>
      )}
    </div>
  );
};

export default AudioTranscription;
