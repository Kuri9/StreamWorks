import React, { ButtonHTMLAttributes } from "react";
import { IconBaseProps } from "react-icons/lib";
export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
    type?: "button" | "submit" | "reset" | undefined;
    label?: string;
    variant?: "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark" | "link";
    icon?: React.ComponentType<IconBaseProps>;
}
export declare const Button: React.FC<ButtonProps>;
//# sourceMappingURL=index.d.ts.map