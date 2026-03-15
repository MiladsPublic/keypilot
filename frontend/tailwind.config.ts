import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./src/app/**/*.{ts,tsx}",
    "./src/components/**/*.{ts,tsx}",
    "./src/features/**/*.{ts,tsx}",
    "./src/lib/**/*.{ts,tsx}"
  ],
  theme: {
    extend: {
      colors: {
        canvas: "var(--canvas)",
        ink: "var(--ink)",
        mist: "var(--mist)",
        accent: "var(--accent)",
        line: "var(--line)"
      },
      boxShadow: {
        panel: "0 18px 50px -24px rgba(38, 53, 42, 0.28)"
      },
      borderRadius: {
        panel: "1.5rem"
      }
    }
  },
  plugins: []
};

export default config;
