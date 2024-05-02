import { useState, useEffect } from 'react';
import axios from 'axios';

const ADDRESS_API_BASE_URL = import.meta.env.VITE_API_URL + 'user/address';

const useAddress = () => {
  const [addresses, setAddresses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchAddresses = async () => {
      try {
        const response = await axios.get(ADDRESS_API_BASE_URL);
        console.log(response.data);
        setAddresses(response.data);
        setLoading(false);
      } catch (error) {
        setError(error);
        setLoading(false);
      }
    };

    fetchAddresses();
  }, []);

  const addAddress = async (address) => {
    try {
      await axios.post(ADDRESS_API_BASE_URL, address);

      const response = await axios.get(ADDRESS_API_BASE_URL);
      setAddresses(response.data);
    } catch (error) {
      setError(error);
    }
  };

  const deleteAddress = async (addressId) => {
    console.log(addressId);
    try {
      await axios.delete(ADDRESS_API_BASE_URL + '/' + addressId);
      setAddresses(addresses.filter((address) => address.id !== addressId));
    } catch (error) {
      setError(error);
    }
  };

  const updateAddress = async (address) => {
    try {
      await axios.put(ADDRESS_API_BASE_URL, address);
      setAddresses(addresses.map((a) => (a.id === address.id ? address : a)));
    } catch (error) {
      setError(error);
    }
  };

  return {
    addresses,
    loading,
    error,
    addAddress,
    deleteAddress,
    updateAddress,
  };
};

export default useAddress;
