import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { LoginDto,RegisterDto } from "../types/auth";
import agent from "../api/agent";
import { useNavigate } from "react-router";

export const useAuth = () => {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const loginAsync = useMutation({
    mutationFn: async (creds: LoginDto) => {
      const response = await agent.post("/auth/login", creds);
      return response.data;
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["currentUser"] });
      navigate("/dashboard");
    },
  });
  const registerAsync = useMutation({
      mutationFn: async (creds: RegisterDto) => {
            const response = await agent.post("/auth/register",creds)
            return response.data;
        }
    }) 
  const logoutAsync = useMutation({
      mutationFn: async () => {
      await agent.post("/auth/logout");
    },
    onSuccess: async () => {
      await queryClient.removeQueries({ queryKey: ["currentUser"] });
      navigate("/");
    },
   });
  return {
    loginAsync,
    registerAsync,
    logoutAsync,
  };
};