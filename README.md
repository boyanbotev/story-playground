# Story Playground
A RESTful Web App using ASP.NET Core in .NET 10.0, implementing node-based user story creation. Features an LLM-powered play mode where story text is created on the fly according to user created structure and concept.

## Tech Stack
### Backend
- .NET 10 / ASP.NET Core Web App
- [ASP.NET Core Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-10.0&tabs=visual-studio) for Authorization and Authentication
- [SQLite](https://sqlite.org/) Database with [Entity Framework Core](https://learn.microsoft.com/en-us/ef/)
- [xUnit](https://xunit.net/?tabs=cs) for testing
- [Ollama](https://ollama.com/) to interface with LLMs
### Frontend
- [React](https://react.dev/) for reusable components
- [Vite.js](https://vite.dev/) as a bundler
- [TypeScript](https://www.typescriptlang.org/) for type safety
- [Material UI](https://mui.com/material-ui/) for styling

## Prerequisites

- .NET 10 SDK
- Entity Framework Core CLI

## Features
- Create your own stories in the story editor using
   * Story nodes with preplanned events
   * Quests where the user can achieve a goal using their own ingenuity and their own ideas
- Play the stories using an LLM running free and more environmentally friendly on your own computer
