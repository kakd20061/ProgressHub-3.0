/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      backgroundImage: {
        grass: "url('assets/images/background.png')",
      },
      backgroundColor: {
        primary: "#ECF1CC",
        disabled: "#ECECEC",
      },
      colors: {
        primary: "#B4C734",
        secondary: "#637200",
        tertiary: "#5F6C74",
        invalid: "#F32424",
        disabled: "#BCBCBC",
      },
      borderColor: {
        button: "#5F6C74",
        invalid: "#F32424",
      },
      borderWidth: {},
      backdropBlur: {
        xs: "2px",
      },
      fontSize: {
        logo: "44px",
        header: "49px",
        normal: "23px",
        icon: "150px",
      },
      fontFamily: {
        lobster: ['"Lobster"', "sans-serif"],
        outfit: ['"Outfit"', "sans-serif"],
      },
      width: {
        sidepanel: "450px",
        sidepanelwide: "calc(100% - 450px)",
        verification: "600px",
      },
      height: {
        verification: "569px",
      },
    },
  },
  plugins: [],
};
