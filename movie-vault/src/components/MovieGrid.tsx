import { useMovies } from '../hooks/useMovies';
import { MovieCard } from './MovieCard';

export const MovieGrid = () => {
  const { data: movies, isLoading, error } = useMovies();

  if (isLoading) return (
    <div className="flex h-64 items-center justify-center">
      <div className="h-12 w-12 animate-spin rounded-full border-4 border-blue-500 border-t-transparent"></div>
    </div>
  );

  if (error) return (
    <div className="rounded-md bg-red-50 p-4 text-red-700">
      Error loading movies: {error instanceof Error ? error.message : 'Unknown error'}
    </div>
  );

  return (
    <div className="container mx-auto p-6">
      <h1 className="mb-8 text-3xl font-bold text-white">My Movie Vault</h1>
      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
        {movies?.map((movie) => (
          <MovieCard key={movie.id} movie={movie} />
        ))}
      </div>
    </div>
  );
};