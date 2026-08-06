import { Button } from "@mui/material";
import LogoutIcon from "@mui/icons-material/Logout";
import { useAuth } from "../../lib/hooks/useAuth";

export default function LogoutButton() {
  const { logoutAsync } = useAuth();

  return (
    <Button
      onClick={() => logoutAsync.mutate()}
      disabled={logoutAsync.isPending}
      variant="outlined"
      startIcon={<LogoutIcon />}
    >
      {logoutAsync.isPending ? "Logging out…" : "Log out"}
    </Button>
  );
}
