import React, { useState } from 'react';
import { FormControl, FormLabel, Input, Button } from '@chakra-ui/react';

const AddressForm = ({
  newAddress,
  handleAddressChange,
  handleAddAddress,
  onClose,
}) => {
  return (
    <>
      <FormControl mb={4} isRequired>
        <FormLabel>Місто</FormLabel>
        <Input
          name='city'
          value={newAddress.city}
          onChange={handleAddressChange}
        />
      </FormControl>
      <FormControl mb={4} isRequired>
        <FormLabel>Вулиця</FormLabel>
        <Input
          name='street'
          value={newAddress.street}
          onChange={handleAddressChange}
        />
      </FormControl>
      <FormControl mb={4} isRequired>
        <FormLabel>Будинок</FormLabel>
        <Input
          name='building'
          value={newAddress.building}
          onChange={handleAddressChange}
        />
      </FormControl>
      <FormControl mb={4}>
        <FormLabel>Квартира (необов'язково)</FormLabel>
        <Input
          name='apartment'
          value={newAddress.apartment}
          onChange={handleAddressChange}
        />
      </FormControl>
      <FormControl mb={4}>
        <FormLabel>Область</FormLabel>
        <Input
          name='region'
          value={newAddress.region}
          onChange={handleAddressChange}
        />
      </FormControl>
      <Button colorScheme='blue' mr={3} onClick={handleAddAddress}>
        ОК
      </Button>
      <Button variant='ghost' onClick={onClose}>
        Скасувати
      </Button>
    </>
  );
};

export default AddressForm;
