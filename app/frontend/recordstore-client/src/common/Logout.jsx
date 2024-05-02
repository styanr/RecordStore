import { useEffect } from 'react';
import useAuth from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';

import { Spinner } from '@chakra-ui/react';

const Logout = () => {
  const { logout } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    logout();
    navigate('/');
  }, []);

  return <Spinner />;
};

export default Logout;
