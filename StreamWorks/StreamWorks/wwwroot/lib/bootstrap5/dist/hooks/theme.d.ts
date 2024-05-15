import React from "react";
import { DefaultTheme } from "styled-components";
declare type ThemeContextData = {
    theme: DefaultTheme;
    setTheme: (t: DefaultTheme) => void;
};
declare const ThemeProvider: React.FC;
declare function useTheme(): ThemeContextData;
export { ThemeProvider, useTheme };
//# sourceMappingURL=theme.d.ts.map