import axios from 'axios';

import { useState, useEffect } from 'react';

const api_url = import.meta.env.VITE_API_URL + 'users';

import useAuth from './useAuth';

const useUsers = () => {
  const [users, setUsers] = useState([]);
  const [roles, setRoles] = useState([]);
  const [rolesLoading, setRolesLoading] = useState(true);

  const [params, setParams] = useState({});
  const [loading, setLoading] = useState(true);

  const { isAuthenticated } = useAuth();

  // params: email, roleName, name, page, orderBy ("email", "role"), orderDirection ("asc", "desc")
  const fetchUsers = async (params) => {
    setLoading(true);
    const response = await axios.get(api_url, { params });
    setUsers(response.data);
    setLoading(false);
  };

  const fetchRoles = async () => {
    setRolesLoading(true);
    const response = await axios.get(api_url + '/roles');
    setRoles(response.data);
    setRolesLoading(false);
  };

  const updateRole = async (userId, roleId) => {
    try {
      await axios.put(api_url + '/' + userId + '/role', { roleId });
      fetchUsers(params);
      return { success: true };
    } catch (error) {
      console.log(error);
      return { success: false, error: error.response.data.message };
    }
  };

  const deleteUser = async (userId) => {
    try {
      await axios.delete(api_url + '/' + userId);
      fetchUsers(params);
      return { success: true };
    } catch (error) {
      console.log(error);
      return { success: false, error: error.response.data.message };
    }
  };

  // user: email, password, firstName, lastName, roleId
  const createUser = async (user) => {
    try {
      await axios.post(import.meta.env.VITE_API_URL + 'auth/create', user);

      fetchUsers(params);
      return { success: true };
    } catch (error) {
      console.log(error);
      return { success: false, error: error.response.data.message };
    }
  };

  useEffect(() => {
    if (isAuthenticated === undefined) return;
    fetchUsers(params);
    fetchRoles();
  }, [isAuthenticated]);

  useEffect(() => {
    console.log('fetching users', params);
    fetchUsers(params);
  }, [params]);

  return {
    users,
    params,
    setParams,
    loading,
    roles,
    rolesLoading,
    updateRole,
    deleteUser,
    createUser,
  };
};

export default useUsers;
