import { createBrowserRouter, Navigate } from "react-router";
import App from "../App";
import ErrorPage from "../../features/errors/ErrorPage";
import NotFound from "../../features/errors/NotFound";
import LoginForm from "../../features/auth/LoginForm";
import RegisterForm from "../../features/auth/RegisterForm";


export const routes = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    errorElement: <ErrorPage />,
    children: [
      { index: true, element: <Navigate to="/" replace /> },
      { path: "login", element: <LoginForm /> },
      { path: "register", element: <RegisterForm /> },

    ],
  },
  { path: "*", element: <NotFound /> },
]);