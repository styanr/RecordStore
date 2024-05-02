import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'artists';

const useArtists = () => {
  // artist:
  // id: number
  // name: string
  const getArtists = async (params) => {
    const response = await axios.get(api_url, { params });
    return response.data;
  };

  // artist:
  // id: number
  // name: string
  // description: string
  const getArtist = async (id) => {
    const response = await axios.get(api_url + '/' + id);
    return response.data;
  };
  // artist:
  // name: string
  // description: string
  const createArtist = async (artist) => {
    const response = await axios.post(api_url, artist);
    return response.data;
  };

  // artist:
  // name: string
  // description: string
  const updateArtist = async (id, artist) => {
    const response = await axios.put(api_url + '/' + id, artist);
    return response.data;
  };

  const deleteArtist = async (id) => {
    await axios.delete(api_url + '/' + id);
  };

  return { getArtists, getArtist, createArtist, updateArtist, deleteArtist };
};

export default useArtists;
