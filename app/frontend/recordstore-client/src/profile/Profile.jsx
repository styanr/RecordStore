import React, { useEffect, useState } from 'react';
import { Flex, Box, Avatar, Text, Button } from '@chakra-ui/react';
import CustomBadge from './CustomBadge';
import useAuth from '../auth/useAuth';
import useAddress from './useAddress';
import stringToColor from '../utils/stringToColor';
import AddressSection from './AddressSection';

import { HiX } from 'react-icons/hi';

import { useNavigate } from 'react-router-dom';

export default function Profile() {
  const { user } = useAuth();
  const [roleColor, setRoleColor] = useState('gray');
  const { addresses, addAddress, deleteAddress } = useAddress();

  const navigate = useNavigate();

  useEffect(() => {
    console.log(user);
    if (user === null) return;
    setRoleColor(stringToColor(user.role));
  }, [user]);

  return (
    <Flex justify='center' align='start' h='100vh'>
      {user ? (
        <Box p={6} m={20} w='100%'>
          <Flex justify='space-between' align='center' mb={4}>
            <Flex align='center' mb={4}>
              <Avatar size='lg' mr={4} />
              <Box>
                <Text fontSize='xl' fontWeight='bold'>
                  {user.firstName} {user.lastName}
                </Text>
                <CustomBadge color={roleColor}>{user.role}</CustomBadge>
              </Box>
            </Flex>
            <Button
              onClick={() => {
                navigate('/logout');
              }}
              colorScheme='red'
              leftIcon={<HiX />}
            >
              Logout
            </Button>
          </Flex>
          {user.role === 'user' && (
            <AddressSection
              addresses={addresses}
              addAddress={addAddress}
              deleteAddress={deleteAddress}
            />
          )}
        </Box>
      ) : (
        <Text>Loading...</Text>
      )}
    </Flex>
  );
}
