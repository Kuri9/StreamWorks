import React, { createContext, useContext, useState } from "react";
import { ThemeProvider as Theme } from "styled-components";
import { darkTheme } from "../themes/dark";
import { lightTheme } from "../themes/light";
const ThemeContext = createContext({});
const ThemeProvider = ({ children }) => {
    const [theme, setTheme] = useState(() => {
        const storedTheme = localStorage.getItem("@theme");
        return storedTheme === "dark" ? darkTheme : lightTheme;
    });
    return (React.createElement(ThemeContext.Provider, { value: { theme, setTheme } },
        React.createElement(Theme, { theme: theme }, children)));
};
function useTheme() {
    const context = useContext(ThemeContext);
    if (!context) {
        throw new Error("useTheme must be used within an ThemeProvider");
    }
    return context;
}
export { ThemeProvider, useTheme };
//# sourceMappingURL=theme.js.map