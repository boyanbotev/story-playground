import { useState } from "react";
import { useNavigate } from "react-router";
import { login } from "../requests/login";
import Box from "@mui/material/Box";
import { FormControl, FormLabel, TextField, Container, Typography, Card } from "@mui/material";
import { StyledButton } from "../components/StyledButton";

export const Login = () => {
  const navigate = useNavigate();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    login(userName, password).then((response) => {
      const token = response.token;
      if (token) {
        localStorage.setItem("token", token);
        navigate("/");
      } else {
        setError(response.error);
      }
    });
  }

  function handleUserNameChange(event: React.ChangeEvent<HTMLInputElement>) {
    setUserName(event.target.value);
  }

  function handlePasswordhange(event: React.ChangeEvent<HTMLInputElement>) {
    setPassword(event.target.value);
  }

  return (
    <Container maxWidth="sm">
      <Card className="sign-in-card">
          <Typography
            component="h1"
            variant="h4"
            sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
          >
            Sign in
        </Typography>
        <Box
              component="form"
              onSubmit={handleSubmit}
              noValidate
              sx={{
                display: 'flex',
                flexDirection: 'column',
                width: '100%',
                gap: 2,
              }}
            >
          {error ? <p className={"error"}>{error}</p> : null}
          <FormControl>
            <FormLabel>
              Username
            </FormLabel>
            <TextField 
              value={userName}
              onChange={handleUserNameChange} 
              placeholder="Enter Username"
            />
          </FormControl>

          <FormControl>
            <FormLabel>
              Password
            </FormLabel>
            <TextField 
              value={password} onChange={handlePasswordhange} 
                placeholder="••••••"
                type="password"
                id="password"
            />
          </FormControl>
          <StyledButton
            type="submit" fullWidth
            variant="contained"
            value="Submit"
          >
            Submit
          </StyledButton>
        </Box>
      </Card>
    </Container>
  );
}
