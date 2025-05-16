import React from "react";
import "./App.css";
import RaceDashboard from "./RaceDashboard";
import { RaceProvider } from "./RaceContext";

const App: React.FC = () => {
  return (
    <div className="App">
      <header className="App-header">
        <h1>AI Race Engineer - Audio Transcription</h1>
      </header>
      <main>
        <RaceProvider>
          <RaceDashboard />
        </RaceProvider>
      </main>
    </div>
  );
};

export default App;
