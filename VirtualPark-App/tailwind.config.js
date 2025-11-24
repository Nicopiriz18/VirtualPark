/** @type {import('tailwindcss').Config} */
module.exports = {
  // Disable Tailwind's preflight (base CSS reset) because it conflicts with
  // Angular Material global styles. We keep all other Tailwind features enabled.
  corePlugins: {
    preflight: false,
  },
  content: ['./src/**/*.{html,ts}'],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#1976d2',
          light: '#63a4ff',
          dark: '#004ba0',
        },
        accent: '#ffb300',
        neutral: '#1f2933',
        'surface-light': '#f5f7fb',
      },
    },
  },
  plugins: [],
};
