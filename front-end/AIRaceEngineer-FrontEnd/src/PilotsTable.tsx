import React, { useEffect, useState } from "react";
import axios from "axios";

type Driver = {
  id: number;
  abbreviation: string;
  name: string;
  team: string;
};

type DriverWithGap = {
  driver: Driver;
  gapFromLeader: string;
  isUser: boolean;
};

type Props = {
  drivers: DriverWithGap[];
  setDrivers: React.Dispatch<React.SetStateAction<DriverWithGap[]>>;
};

const RaceTable: React.FC<Props> = ({ drivers, setDrivers }) => {
  const userAbbreviation = "YOU";

  useEffect(() => {
    const fetchDrivers = async () => {
      try {
        const response = await axios.get<Driver[]>(
          "https://localhost:7028/api/Pilots"
        );
        response.data.push({
          id: 0,
          abbreviation: "YOU",
          name: "You",
          team: "Your Team",
        });
        const initialDrivers = response.data.map((driver, index) => ({
          driver,
          gapFromLeader:
            index === 0 ? "First" : `${(Math.random() * 30 + 1).toFixed(1)}`,
          isUser: driver.abbreviation === userAbbreviation,
        }));

        setDrivers(sortDrivers(initialDrivers));
      } catch (error) {
        console.error("Error fetching drivers:", error);
      }
    };

    fetchDrivers();
  }, []);

  const sortDrivers = (driverList: DriverWithGap[]) => {
    return [...driverList].sort((a, b) => {
      const gapA =
        a.gapFromLeader === "First" ? 0 : parseFloat(a.gapFromLeader);
      const gapB =
        b.gapFromLeader === "First" ? 0 : parseFloat(b.gapFromLeader);
      return gapA - gapB;
    });
  };

  const handleGapChange = (index: number, value: string) => {
    const updated = [...drivers];
    updated[index].gapFromLeader = value === "" ? "0" : value;
    setDrivers(sortDrivers(updated));
  };

  const handleUserNameChange = (index: number, value: string) => {
    const updated = [...drivers];
    updated[index].driver.name = value;
    setDrivers(updated);
  };

  return (
    <div className="p-4 max-w-xl mx-auto">
      <h2 className="text-2xl font-bold mb-4">Live Race Table</h2>
      <table className="w-full border">
        <thead>
          <tr className="bg-gray-800 text-white">
            <th className="p-2 text-center">Pos</th>
            <th className="p-2 text-center">Team</th>
            <th className="p-2 text-center">Abbr</th>
            <th className="p-2 text-center">Gap to Leader</th>
          </tr>
        </thead>
        <tbody className="bg-gray-600 text-white">
          {drivers.map((entry, index) => (
            <tr key={entry.driver.id} className="border-t">
              <td className="p-2">{index + 1}</td>
              <td className="p-2">
                {entry.isUser ? (
                  <input
                    className="border rounded px-2 text-center bg-white text-black"
                    value={entry.driver.team}
                    onChange={(e) =>
                      handleUserNameChange(index, e.target.value)
                    }
                  />
                ) : (
                  entry.driver.team
                )}
              </td>
              <td className="p-2">{entry.driver.abbreviation}</td>
              <td className="p-2">
                {index === 0 ? (
                  "First"
                ) : (
                  <input
                    type="text"
                    className="border rounded px-2 w-24 text-center"
                    value={entry.gapFromLeader}
                    onChange={(e) => handleGapChange(index, e.target.value)}
                  />
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default RaceTable;
