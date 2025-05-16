import React, { useState } from "react";
import RaceTable from "./PilotsTable";
import AudioTranscription from "./AudioTranscription";
import type { DriverWithGap } from "./RaceContext";

const RaceDashboard: React.FC = () => {
  const [drivers, setDrivers] = useState<DriverWithGap[]>([]);

  return (
    <div>
      <RaceTable drivers={drivers} setDrivers={setDrivers} />
      <AudioTranscription drivers={drivers} />
    </div>
  );
};

export default RaceDashboard;
