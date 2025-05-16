import React, { createContext, useState, useContext } from "react";

export type Driver = {
  id: number;
  abbreviation: string;
  name: string;
  team: string;
};

export type DriverWithGap = {
  driver: Driver;
  gapFromLeader: string;
  isUser: boolean;
};

type RaceContextType = {
  drivers: DriverWithGap[];
  setDrivers: React.Dispatch<React.SetStateAction<DriverWithGap[]>>;
};

const RaceContext = createContext<RaceContextType | undefined>(undefined);

export const RaceProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [drivers, setDrivers] = useState<DriverWithGap[]>([]);
  return (
    <RaceContext.Provider value={{ drivers, setDrivers }}>
      {children}
    </RaceContext.Provider>
  );
};

export const useRaceContext = () => {
  const context = useContext(RaceContext);
  if (!context) {
    throw new Error("useRaceContext must be used within a RaceProvider");
  }
  return context;
};
