var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) {
            if (e.indexOf(p[i]) < 0 && Object.prototype.propertyIsEnumerable.call(s, p[i]))
                t[p[i]] = s[p[i]];
        }
    return t;
};
import React from "react";
import { Container } from "./styles";
export const Button = (_a) => {
    var { type = "button", label, variant = "primary", icon: Icon } = _a, rest = __rest(_a, ["type", "label", "variant", "icon"]);
    return (React.createElement(Container, Object.assign({ label: !!label, variant: variant, className: "btn", type: type }, rest),
        Icon && React.createElement(Icon, null),
        label));
};
//# sourceMappingURL=index.js.map