import { useState, useEffect } from 'react';
import './App.css'

function App() {

  const [ data, setData ] = useState<string>();
  const getApiData = async () => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}`);
    const txt = await response.text();
    return await txt;
  }

  useEffect(() => {
    getApiData().then(data => setData(data));
  }, []);

  return (
    <>
      <h1>Vite + React + .NET</h1>
      <div className="card">
        <p>
          {data}
        </p>
      </div>
    </>
  )
}

export default App
