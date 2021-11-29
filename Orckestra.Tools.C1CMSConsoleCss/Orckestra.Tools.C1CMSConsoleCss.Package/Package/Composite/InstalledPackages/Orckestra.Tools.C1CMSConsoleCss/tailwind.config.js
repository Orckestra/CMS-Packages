const colors = require('tailwindcss/colors')

module.exports = {
  purge: [],
  darkMode: false, // or 'media' or 'class'
  theme: {
    colors: {
      transparent: 'transparent',
      current: 'currentColor',
      black: colors.black,
      white: colors.white,
      gray: colors.trueGray,
      green: colors.green,
    },
    extend: {},
  },
  variants: {
    extend: {
      opacity: ['disabled']
    },
  },
  plugins: [
    require('@tailwindcss/forms')
  ],
}
