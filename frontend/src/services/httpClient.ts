import axios, { AxiosError } from 'axios';
import type { ProblemDetails } from '@/types/api';

const baseURL = import.meta.env.VITE_API_BASE_URL || '';

export const httpClient = axios.create({
  baseURL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

export function extractApiErrorMessage(error: unknown): string {
  if (error instanceof AxiosError) {
    const problem = error.response?.data as ProblemDetails | undefined;
    if (problem?.errors) {
      const messages = Object.values(problem.errors).flat();
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }
    if (problem?.detail) {
      return problem.detail;
    }
    if (problem?.title) {
      return problem.title;
    }
    if (error.message) {
      return error.message;
    }
  }

  if (error instanceof Error) {
    return error.message;
  }

  return 'An unexpected error occurred.';
}
