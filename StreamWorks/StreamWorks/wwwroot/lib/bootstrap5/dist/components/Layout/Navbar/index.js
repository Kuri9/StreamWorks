import React from "react";
import { useCallback } from "react";
import { FaSignOutAlt, FaSun } from "react-icons/fa";
import { Button } from "../../..";
import { useTheme } from "../../../hooks/theme";
import { darkTheme } from "../../../themes/dark";
import { lightTheme } from "../../../themes/light";
import { Container, Logo, Space, Tools } from "./styles";
export const Navbar = ({ logo, signOut }) => {
    const { theme, setTheme } = useTheme();
    const changeTheme = useCallback(() => {
        if (theme === lightTheme) {
            setTheme(darkTheme);
            localStorage.setItem("@theme", "dark");
        }
        else {
            setTheme(lightTheme);
            localStorage.setItem("@theme", "light");
        }
    }, [theme, setTheme]);
    return (React.createElement(Container, null,
        React.createElement(Logo, null, logo),
        React.createElement(Tools, null,
            React.createElement(Button, { variant: "link", type: "button", label: "", icon: FaSun, onClick: changeTheme }),
            React.createElement(Space, null),
            React.createElement(Button, { variant: "danger", type: "button", label: "", icon: FaSignOutAlt, onClick: signOut }))));
};
//# sourceMappingURL=index.js.map