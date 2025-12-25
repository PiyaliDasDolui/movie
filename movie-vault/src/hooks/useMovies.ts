import { useQuery } from '@tanstack/react-query';

export interface Movie {
  id: number;
  title: string;
  releaseDate: string;
  posterPath: string;
  overview: string;
}

export const useMovies = () => {
  return useQuery<Movie[]>({
    queryKey: ['movies'],
    queryFn: async () => {
      const response = await fetch('http://localhost:5064/api/movies');
      if (!response.ok) throw new Error('Network response was not ok');
      return response.json();
    },
  });
};