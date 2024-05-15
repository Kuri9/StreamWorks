import React from "react";
import { ThemeProvider as Theme } from "../hooks/theme";
export const ThemeProvider = ({ children }) => {
    return React.createElement(Theme, null, children);
};
//# sourceMappingURL=index.js.map