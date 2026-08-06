import { Outlet } from "react-router";
import { Box } from "@mui/material";
import Footer from "./Footer";
import Header from "./Header"
function App() {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
      <Header />
      <Box sx={{ height: { xs: 56, md: 64 } }} />

      <Box component="main" sx={{ flex: 1 }}>
        <Outlet />
  
      </Box>

      <Footer />
    </Box>
  );
}

export default App;