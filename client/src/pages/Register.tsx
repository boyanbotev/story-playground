import { useState } from "react";
import { useNavigate } from "react-router";
import { register } from "../requests/register";
import { login } from "../requests/login";
import Box from "@mui/material/Box";
import { FormControl, FormLabel, TextField, Container, Typography, Card } from "@mui/material";
import { StyledButton } from "../components/StyledButton";

export const Register = () => {
  const navigate = useNavigate();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [apiKey, setApiKey] = useState("");
  const [errors, setErrors] = useState([""]);

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    register(userName, password, apiKey).then((response) => {
      const token = response.token;
      if (token) {
        login(userName, password).then((response) => {
          const token = response.token;
          localStorage.setItem("token", token);
          navigate("/");
        });
      } else {
        setErrors(response.errors);
      }
    });
  }

  function handleUserNameChange(event: React.ChangeEvent<HTMLInputElement>) {
    setUserName(event.target.value);
  }

  function handlePasswordhange(event: React.ChangeEvent<HTMLInputElement>) {
    setPassword(event.target.value);
  }

  function handleApiKeyChange(event: React.ChangeEvent<HTMLInputElement>) {
    setApiKey(event.target.value);
  }

  return (
    <Container maxWidth="sm">
      <Card className="sign-in-card">
          <Typography
            component="h1"
            variant="h4"
            sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
          >
          Register
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
          {errors?.length ? errors.map((error, index) => <p key={index} className={"error"}>{error}</p>) : null}
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

          <FormControl>
            <FormLabel>
              Google AI API Key
            </FormLabel>
            <TextField 
              value={apiKey}
              onChange={handleApiKeyChange} 
              placeholder="Enter Google AI API key"
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
