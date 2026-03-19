import { useState, useEffect } from 'react';
import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router/dom';
import './App.css'
import { Stories } from './pages/Stories';
import { Edit } from './pages/Edit';
import { Game } from './pages/Game';
import { Add } from './pages/Add';
import { RootLayout } from './components/RootLayout';
import { fetchStory } from './requests/fetchStory';
import { fetchStories } from './requests/fetchStories';

function App() {

  const [ data, setData ] = useState<string>();

  const getApiData = async () => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\stories`);
    const txt = await response.text();
    return await txt;
  }

  const submitStory = async () => {
    const baseUrl = import.meta.env.VITE_API_BASE_URL;
    const response = await fetch(`${baseUrl}\stories`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        name: 'test',
        structure: 'test',
        nodes: [
          {
            content: 'test',
            turns: 1,
          },
        ],
      }),
    });
  }

  useEffect(() => {
    getApiData().then(data => setData(data));
  }, []);

  const router = createBrowserRouter([
    {
      path: "/",
      Component: RootLayout,
      children: [
        {
          index: true,
          element: <h1>Story Playground</h1>,
        },
        {
          path: "stories",
          Component: Stories,
            loader: async () => {
            let stories = await fetchStories();
            return { stories };
          }
        },
        {
          path: "stories/add",
          Component: Add,
        },
        {
          path: "stories/:storyId",
          Component: Edit,
          loader: async ({ params }) => {
            if (params.storyId == null) return;
            let story = await fetchStory(parseInt(params.storyId));
            return { story };
          }
        },
        {
          path: "stories/:storyId/play",
          Component: Game,
        }
      ]
    },

  ]);

  return (
    <>
      {/*<button onClick={submitStory}>Submit</button> */}
      <RouterProvider router={router} />
    </>
  )
}

export default App
