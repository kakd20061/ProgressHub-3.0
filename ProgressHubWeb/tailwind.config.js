/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}",
    "./node_modules/flowbite/**/*.js"],

  theme: {
    extend: {
      backgroundImage: {
        grass: "url('assets/images/background.png')",
      },
      screens: {
        xs: "400px",
      },
      backgroundColor: {
        primary: "#ECF1CC",
        disabled: "#ECECEC",
      },
      colors: {
        mprimary: "#3FA99B",
        msecondary: "#45C6A3",
        mtertiary: "#7ECFB6",
        mlight: "#D5F4EE",
        msemilight: "#C4EFE6",
        msemidark: "#8AD6CC",
        tertiary: "#5F6C74",
        invalid: "#F32424",
        disabled: "#BCBCBC",
        reddark: "#B30000",
        redprimary: "#d51919",
        redsecondary: "#FF4D4D",
        redtertiary: "#FF8080",
        redlight: "#FFCCCC",
        redsemilight: "#FFEBEB",
        redsemidark: "#FF9999",
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

      keyframes: {
        sidebar: {
          '0%': { left: '-100%' },
          '100%': { left: '0' },
        },
        sidebar2: {
          '0%': { left: '0' },
          '100%': { left: '-100%' },
        },
      },
      animation: {
        'open-sidebar': 'sidebar 0.3s linear',
        'close-sidebar': 'sidebar2 0.3s linear',
      },
    },
  },
  plugins: [
    require('flowbite/plugin'),
  ]
};
