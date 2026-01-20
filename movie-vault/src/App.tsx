import { MovieGrid } from './components/MovieGrid';
import CreateMovie from './components/CreateMovie';

function App() {
  return (
    <div className="min-h-screen bg-gray-950 text-white">
      <nav className="border-b border-gray-800 p-4">
        <span className="text-xl font-bold text-blue-500">🎬 MovieVault</span>
      </nav>
      <main>
        {/* <CreateMovie /> */}
        <MovieGrid />
      </main>
    </div>
  );
}

export default App;