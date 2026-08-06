import { useState } from "react";
import { useAuth } from "../../lib/hooks/useAuth";
import type { RegisterDto } from "../../lib/types/auth";
import { Container,CircularProgress,Alert, Box, Paper, Typography, TextField, InputAdornment, IconButton, Button, Stack } from "@mui/material";
import { Visibility, VisibilityOff } from "@mui/icons-material";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";

export default function RegisterForm() {
  const { registerAsync } = useAuth();
  const {register,handleSubmit, reset,resetField, formState: {errors}} = useForm<RegisterDto>({
    defaultValues: {username: "", email: "", password: ""}
  })
  const [showPassword, setShowPassword] = useState(false);

  const onSubmit = (creds: RegisterDto) => {
    registerAsync.mutateAsync(creds,{
        onSuccess: () =>{
            reset();
        },
        onError: () => {
            resetField("password")
        }
    });
  };
  const navigate = useNavigate();
  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: "100vh",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <Paper sx={{ p: 4, width: "100%" }}>
          <Typography
            variant="h3"
            sx={{
              m: 2,
              textAlign: "center",
            }}
          >
            Register
          </Typography>

          <Box component="form" onSubmit={handleSubmit(onSubmit)}>
            <Stack spacing={2}>
              <TextField
                label="UserName"
                fullWidth
                {...register("username", { required: "Username is required" })}
                error={!!errors.username}
                helperText={errors.username?.message}
              />
              <TextField
                label="Email"
                type="email"
                fullWidth
                {...register("email", { required: "Email is required" })}
                error={!!errors.email}
                helperText={errors.email?.message}
              />
              <TextField
                label="Password"
                type={showPassword ? "text" : "password"}
                {...register("password", {
                  required: "Password is required",
                  minLength: {
                    value: 8,
                    message: "Must be at least 8 characters",
                  },
                })}
                error={!!errors.password}
                helperText={errors.password?.message}
                fullWidth
                slotProps={{
                  input: {
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => setShowPassword(!showPassword)}
                          edge="end"
                        >
                          {showPassword ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  },
                }}
              />
              <Button
                type="submit"
                variant="contained"
                fullWidth
                disabled={registerAsync.isPending}
              >
                {registerAsync.isPending ? (
                  <CircularProgress size={24} color="inherit" />
                ) : (
                  "Register"
                )}
              </Button>
              {registerAsync.data?.isSuccess && (
                <Alert severity="success">{registerAsync.data.value}</Alert>
              )}
              {registerAsync.error && (
                <Alert severity="error">
                  {registerAsync.error.message}
                </Alert>
              )}
              <Button
                variant="text"
                sx={{ border: 2, m: 1, width: "100%" }}
                onClick={() => navigate("/login")}
              >
                Already registered
              </Button>
            </Stack>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}