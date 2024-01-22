import React, { useState, useEffect } from 'react';
import axios from 'axios';

const App = () => {
  const [operations, setOperations] = useState([]);

  useEffect(() => {
    const fetchOperations = async () => {
      try {
        const response = await axios.get('https://localhost:7159/Controllers');
        setOperations(response.data);
      } catch (error) {
        console.error('Error fetching data: ', error);
      }
    };

    fetchOperations();
  }, []);

  return (
    <div>
      <h1>Operacje</h1>
      <ul>
        {operations.map(operation => (
          <li key={operation.id}>
            Data: {operation.operationDate},Godzina {operation.operationTime}, Kwota: {operation.operationAmount}, Opis: {operation.description}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default App;

