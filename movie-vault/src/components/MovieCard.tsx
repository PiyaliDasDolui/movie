// 1. Import the Interface we created in Phase 3
import { Movie } from '../hooks/useMovies';

// 2. Define the Props interface specifically for this component
interface MovieCardProps {
  movie: Movie;
}

// 3. Use the interface to "type" the component
export const MovieCard = ({ movie }: MovieCardProps) => {
  return (
    <div className="bg-gray-800 rounded-lg overflow-hidden shadow-lg transition-transform hover:scale-105">
      <img 
        src={movie.posterPath} 
        alt={movie.title} 
        className="w-full h-64 object-cover"
      />
      <div className="p-4">
        <h3 className="text-white font-bold text-lg truncate">{movie.title}</h3>
        <p className="text-gray-400 text-sm">{movie.releaseDate}</p>
      </div>
    </div>
  );
};