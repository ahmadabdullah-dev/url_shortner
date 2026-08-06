import axios, { AxiosError } from "axios";

const agent = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    withCredentials: true
});

agent.interceptors.response.use(
  (response) => response, (error: AxiosError<{ error?: string }>) => {
    if (error.response?.data?.error) {
      return Promise.reject(new Error(error.response.data.error));
    }
    return Promise.reject(error);
  }
);
export default agent;