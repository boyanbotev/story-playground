import { useState } from "react";
import { useNavigate } from "react-router";
import { login } from "../requests/login";

export const Login = () => {
  const navigate = useNavigate();
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    login(userName, password).then((response) => {
      const token = response.token;
      localStorage.setItem("token", token);
      navigate("/");
    });
  }

  function handleUserNameChange(event: React.ChangeEvent<HTMLInputElement>) {
    setUserName(event.target.value);
  }

  function handlePasswordhange(event: React.ChangeEvent<HTMLInputElement>) {
    setPassword(event.target.value);
  }

  return (
    <div>
      Login Page
      <form onSubmit={handleSubmit}>
        <label>
          User Name:
          <input type="text" value={userName} onChange={handleUserNameChange} />
        </label>
        <label>
          Password:
          <input type="text" value={password} onChange={handlePasswordhange} />
        </label>
        <input type="submit" value="Submit" />
      </form>
    </div>
  );
}
