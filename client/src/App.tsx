import { createBrowserRouter, data } from 'react-router';
import { RouterProvider } from 'react-router/dom';
import './App.css'
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { Home } from './pages/Home';
import { Stories } from './pages/Stories';
import { Edit } from './pages/Edit';
import { Game } from './pages/Game';
import { Add } from './pages/Add';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { RootLayout } from './components/RootLayout';
import { fetchStory } from './requests/fetchStory';
import { fetchStories } from './requests/fetchStories';
import { RootErrorBoundary } from './components/RouteErrorBoundary';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';

const darkTheme = createTheme({
  palette: {
    mode: 'dark',
  },
});

function App() {

  const router = createBrowserRouter([
    {
      path: "/",
      Component: RootLayout,
      children: [
        {
          index: true,
          Component: Home,
        },
        {
          path: "stories",
          Component: Stories,
          ErrorBoundary: RootErrorBoundary,
            loader: async () => {
              const token = localStorage.getItem("token");

              if (token) {
                let stories = await fetchStories(token!);

                if (!stories) throw data("Stories Not Found", { status: 404 });
                return { stories };
              }
              throw data("Not logged in!", { status: 404 });
          }
        },
        {
          path: "stories/add",
          ErrorBoundary: RootErrorBoundary,
          Component: Add,
        },
        {
          path: "stories/:storyId/edit",
          Component: Edit,
          ErrorBoundary: RootErrorBoundary,
          loader: async ({ params }) => {
            if (params.storyId == null) return;

            let token = localStorage.getItem("token");
            if (token) {
              let story = await fetchStory(parseInt(params.storyId), token);
              if (!story) throw data("Story Not Found", { status: 404 });
              return { story };
            }
            throw data("Not logged in!", { status: 404 });
          }
        },
        {
          path: "stories/:storyId/play",
          Component: Game,
          ErrorBoundary: RootErrorBoundary,
          loader: async ({ params }) => {
            if (params.storyId == null) return;
            let token = localStorage.getItem("token");

            if (token) {
              let story = await fetchStory(parseInt(params.storyId), token);
              if (!story) throw data("Story Not Found", { status: 404 });
              return { story };
            }
            throw data("Not logged in!", { status: 404 });
          }
        },
        {
          path: "register",
          Component: Register,
          ErrorBoundary: RootErrorBoundary,
        },
        {
          path: "login",
          Component: Login,
          ErrorBoundary: RootErrorBoundary,
        }
      ]
    },

  ]);

  return (
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
      <RouterProvider router={router} />
    </ThemeProvider>
  )
}

export default App
