import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL;

const useRecords = () => {
  // id, title, description, releaseDate
  const getRecords = async (params) => {
    const response = await axios.get(`${api_url}records`, { params });

    return response.data;
  };

  // id, title, description, releaseDate, artists: [{id, name}], genres: [{name}]
  const getRecord = async (recordId) => {
    const response = await axios.get(`${api_url}records/${recordId}`);

    return response.data;
  };

  const getRecordsForArtist = async (artistId) => {
    const response = await axios.get(`${api_url}artists/${artistId}/records`);

    return response.data;
  };

  // title, description, releaseDate, artistIds: [int], genreNames: [string]
  const createRecord = async (record) => {
    const response = await axios.post(`${api_url}records`, record);

    return response.data;
  };

  // title, description, releaseDate, artistIds: [int], genreNames: [string]
  const updateRecord = async (recordId, record) => {
    const response = await axios.put(`${api_url}records/${recordId}`, record);

    return response.data;
  };

  const deleteRecord = async (recordId) => {
    const response = await axios.delete(`${api_url}records/${recordId}`);

    return response.data;
  };

  return {
    getRecords,
    getRecord,
    updateRecord,
    getRecordsForArtist,
    createRecord,
    deleteRecord,
  };
};

export default useRecords;
