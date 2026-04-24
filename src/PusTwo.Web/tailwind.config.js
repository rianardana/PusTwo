/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Pages/**/*.cshtml", 
    "./wwwroot/**/*.js",
    "./**/*.cshtml"
  ],
  theme: {
    extend: {
      colors: {
        primary: '#1e40af',
        secondary: '#64748b',
      }
    },
  },
  plugins: [],
}