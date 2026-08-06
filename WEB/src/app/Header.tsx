import * as React from "react";
import {
  AppBar,
  Toolbar,
  Box,
  Typography,
  Stack,
  Link as MuiLink,
} from "@mui/material";
import TemporaryDrawer from "./TemporaryDrawer";
import { useEffect } from "react";
const NAV_LINKS = [
  { label: "Home", href: "/" },
];

export default function Header() {
  const [scrolled, setScrolled] = React.useState(false);

  useEffect(() => {
    const onScroll = () => setScrolled(window.scrollY > 8);
    onScroll();
    window.addEventListener("scroll", onScroll, { passive: true });
    return () => window.removeEventListener("scroll", onScroll);
  }, []);

  return (
    <AppBar
      position="fixed"
      elevation={0}
      sx={(theme) => ({
        backgroundColor: scrolled ? `${theme.palette.background.paper}`: "transparent",
        backdropFilter: scrolled ? "blur(8px)" : "none",
        borderBottom: `1px solid ${scrolled ? theme.palette.divider : "transparent"}`,
        transition: "background-color 200ms ease, border-color 200ms ease",
        boxShadow: "none",
      })}
    >
      <Toolbar
        sx={{
          minHeight: { xs: 56, md: 64 },
          px: { xs: 2, md: 3 },
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <Typography
          component="a"
          href="/"
          sx={{
            color: "text.primary",
            fontWeight: 700,
            fontSize: { xs: 16, md: 18 },
            letterSpacing: "0.22em",
            textDecoration: "none",
            userSelect: "none",
          }}
        >
          URL Shortner
        </Typography>

        <Stack
          direction="row"
          spacing={4}
          sx={{ display: { xs: "none", md: "flex" } }}
        >
          {NAV_LINKS.map((link) => (
            <MuiLink
              key={link.label}
              href={link.href}
              underline="none"
              sx={{
                color: "text.primary",
                fontWeight: 600,
                fontSize: 12,
                letterSpacing: "0.12em",
                textTransform: "uppercase",
                position: "relative",
                pb: 0.5,
                "&:hover": { color: "text.primary" },
                "&:hover::after": { width: "100%" },
                "&::after": {
                  content: '""',
                  position: "absolute",
                  left: 0,
                  bottom: 0,
                  width: 0,
                  height: "1px",
                  backgroundColor: (theme) => theme.palette.secondary.main,
                  transition: "width 150ms ease",
                },
              }}
            >
              {link.label}
            </MuiLink>
          ))}
        </Stack>

        <Box sx={{ display: { xs: "flex", md: "none" }, alignItems: "center" }}>
          <TemporaryDrawer items={NAV_LINKS} />
        </Box>
      </Toolbar>
    </AppBar>
  );
}