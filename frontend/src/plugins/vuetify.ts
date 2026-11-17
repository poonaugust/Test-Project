import '@mdi/font/css/materialdesignicons.css';
import 'vuetify/styles';
import { createVuetify } from 'vuetify';

export default createVuetify({
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        dark: false,
        colors: {
          primary: '#1E40AF',
          secondary: '#0F766E',
          surface: '#FFFFFF',
          background: '#F8FAFC',
          error: '#DC2626',
          warning: '#D97706',
          success: '#16A34A',
        },
      },
      dark: {
        dark: true,
        colors: {
          primary: '#60A5FA',
          secondary: '#2DD4BF',
          surface: '#1E293B',
          background: '#0F172A',
        },
      },
    },
  },
  defaults: {
    VBtn: {
      rounded: 'lg',
    },
    VCard: {
      rounded: 'lg',
      elevation: 1,
    },
    VTextField: {
      variant: 'outlined',
      density: 'comfortable',
    },
    VSelect: {
      variant: 'outlined',
      density: 'comfortable',
    },
    VTextarea: {
      variant: 'outlined',
      density: 'comfortable',
    },
  },
});
