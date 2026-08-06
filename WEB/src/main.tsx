import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { routes } from "./app/router/routes";
import { ThemeProvider, CssBaseline } from "@mui/material";
import theme from './lib/theme';
import { RouterProvider } from "react-router";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
const queryClient = new QueryClient();

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <QueryClientProvider client={queryClient}>
        <RouterProvider router={routes} />
      </QueryClientProvider>
    </ThemeProvider>
  </StrictMode>,
);