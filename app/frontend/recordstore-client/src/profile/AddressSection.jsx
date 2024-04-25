import React, { useState } from 'react';
import {
  Box,
  Button,
  FormControl,
  FormLabel,
  Input,
  List,
  ListItem,
  ListIcon,
  Modal,
  ModalBody,
  ModalContent,
  ModalFooter,
  ModalHeader,
  ModalOverlay,
  Text,
} from '@chakra-ui/react';
import { MdLocationOn } from 'react-icons/md';

import AddressForm from '../common/AddressForm';

export default function AddressSection({
  addresses,
  addAddress,
  deleteAddress,
}) {
  const [isOpen, setIsOpen] = useState(false);
  const [newAddress, setNewAddress] = useState({
    city: '',
    street: '',
    building: '',
    apartment: '',
    region: '',
  });

  const handleAddressChange = (e) => {
    const { name, value } = e.target;
    setNewAddress({ ...newAddress, [name]: value });
  };

  const handleAddAddress = () => {
    addAddress(newAddress);
    setNewAddress({
      city: '',
      street: '',
      building: '',
      apartment: '',
      region: '',
    });
    setIsOpen(false);
  };

  return (
    <Box mb={4}>
      <Text fontSize='xl' fontWeight='bold' mb={2}>
        Адреси
      </Text>
      <List spacing={3}>
        {addresses ? (
          addresses.map((address) => (
            <ListItem
              key={address.id}
              display='flex'
              alignItems='center'
              justifyContent='space-between'
              w='50rem'
              my={5}
              py={4}
              px={6}
              borderRadius='md'
              boxShadow={'md'}
            >
              <Box display='flex' alignItems='center' dir='row' w='100%'>
                <ListIcon as={MdLocationOn} color='green.500' />
                <Box>
                  <Box>
                    <Text fontWeight='bold' as='span'>
                      Область:{' '}
                    </Text>
                    <Text as='span'>{address.region}</Text>
                  </Box>
                  <Box>
                    <Text fontWeight='bold' as='span'>
                      Адреса:{' '}
                    </Text>
                    <Text as='span'>
                      {address.city}, вул. {address.street}, буд.{' '}
                      {address.building}{' '}
                      {address.apartment ? `, кв. ${address.apartment}` : ''}
                    </Text>
                  </Box>
                </Box>
              </Box>
              <Button
                size='sm'
                colorScheme='red'
                onClick={() => deleteAddress(address.id)}
              >
                Видалити
              </Button>
            </ListItem>
          ))
        ) : (
          <Text>Немає адрес</Text>
        )}
      </List>
      <Button onClick={() => setIsOpen(true)}>Додати адресу</Button>
      <Modal isOpen={isOpen} onClose={() => setIsOpen(false)}>
        <ModalOverlay />
        <ModalContent p={4}>
          <ModalHeader>Додати адресу</ModalHeader>
          <ModalBody>
            <AddressForm
              newAddress={newAddress}
              handleAddressChange={handleAddressChange}
              handleSubmit={handleAddAddress}
              onClose={() => setIsOpen(false)}
            />
          </ModalBody>
        </ModalContent>
      </Modal>
    </Box>
  );
}
