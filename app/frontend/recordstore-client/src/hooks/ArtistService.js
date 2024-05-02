import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'artists';

class ArtistService {
  searchArtists = async (name) => {
    const response = await axios.get(api_url, { params: { name } });
    return response.data;
  };
}

const artistService = new ArtistService();
export default artistService;
