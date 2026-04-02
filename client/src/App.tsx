import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router/dom';
import './App.css'
import { Stories } from './pages/Stories';
import { Edit } from './pages/Edit';
import { Game } from './pages/Game';
import { Add } from './pages/Add';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { RootLayout } from './components/RootLayout';
import { fetchStory } from './requests/fetchStory';
import { fetchStories } from './requests/fetchStories';

function App() {

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
              const token = localStorage.getItem("token");

              if (token) {
                let stories = await fetchStories(token!);
                return { stories };
              }
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

            let token = localStorage.getItem("token");
            if (token) {
              let story = await fetchStory(parseInt(params.storyId), token);
              return { story };
            }
          }
        },
        {
          path: "stories/:storyId/play",
          Component: Game,
          loader: async ({ params }) => {
            if (params.storyId == null) return;
            let token = localStorage.getItem("token");

            if (token) {
              let story = await fetchStory(parseInt(params.storyId), token);
              return { story };
            }
          }
        },
        {
          path: "register",
          Component: Register,
        },
        {
          path: "login",
          Component: Login,
        }
      ]
    },

  ]);

  return (
    <>
      <RouterProvider router={router} />
    </>
  )
}

export default App
